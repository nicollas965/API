using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Controllers;

[Route("api/tarefa")]
[ApiController]
public class TarefaController : ControllerBase
{
    private readonly AppDataContext _context;

    public TarefaController(AppDataContext context) =>
    _context = context;

    [HttpGet]
    [Route("listar")]
    public IActionResult Listar()
    {
        try
        {
            List<Tarefa> tarefas = _context.Tarefas.Include(x => x.Categoria).ToList();
            return Ok(tarefas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // GET: api/tarefa/naoconcluidas
    [HttpGet]
    [Route("naoconcluidas")]
    public IActionResult ListarNaoConcluidas()
    {
        try
        {
            // Ajuste para garantir que o status "Não iniciada" e "Em andamento" sejam considerados
            List<Tarefa> tarefasNaoConcluidas = _context.Tarefas
                .Include(x => x.Categoria)
                .Where(x => x.Status != "Concluída") // Verifica se o status não é "Concluída"
                .ToList();
            return Ok(tarefasNaoConcluidas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("concluidas")]
    public IActionResult ListarConcluidas()
    {
        try
        {
            List<Tarefa> tarefas = _context.Tarefas
                .Include(x => x.Categoria)
                .Where(x => x.Status == "Concluída") // Corrigido o erro de digitação aqui
                .ToList();
            return Ok(tarefas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    [Route("cadastrar")]
    public IActionResult Cadastrar([FromBody] Tarefa tarefa)
    {
        try
        {
            Categoria? categoria = _context.Categorias.Find(tarefa.CategoriaId);
            if (categoria == null)
            {
                return NotFound();
            }
            tarefa.Categoria = categoria;
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
            return Created("", tarefa);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch]
    [Route("alterar/{id}")]
    public IActionResult Alterar([FromRoute] int id)
    {
        Tarefa? tarefa = _context.Tarefas.Find(id);
        if (tarefa == null)
        {
            return NotFound();
        }

        if (tarefa.Status == "Não iniciada")
        {
            tarefa.Status = "Em andamento";
        }
        else if (tarefa.Status == "Em andamento") // Adicionado para garantir que o status seja alternado para "Concluída"
        {
            tarefa.Status = "Concluída";
        }

        _context.Tarefas.Update(tarefa);
        _context.SaveChanges(); // Corrigido o erro de sintaxe aqui
        return Ok(tarefa);
    }

    /* 
    app.MapPatch("/api/tarefa/alterar/{id}", async (int id, AppDbContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);
    if (tarefa == null) return Results.NotFound("Tarefa não encontrada.");
    
    tarefa.Status = tarefa.Status switch
    {
        "Não iniciada" => "Em andamento",
        "Em andamento" => "Concluída",
        _ => tarefa.Status
    };
    await db.SaveChangesAsync();
    return Results.Ok(tarefa);
});
GET api/tarefa/naoconcluidas
Retorna tarefas com status "Não iniciada" ou "Em andamento":

csharp
Copiar código
app.MapGet("/api/tarefa/naoconcluidas", async (AppDbContext db) =>
{
    var tarefas = await db.Tarefas
        .Where(t => t.Status == "Não iniciada" || t.Status == "Em andamento")
        .ToListAsync();
    return Results.Ok(tarefas);
});
GET api/tarefa/concluidas
Retorna tarefas com status "Concluída":

csharp
Copiar código
app.MapGet("/api/tarefa/concluidas", async (AppDbContext db) =>
{
    var tarefas = await db.Tarefas
        .Where(t => t.Status == "Concluída")
        .ToListAsync();
    return Results.Ok(tarefas);
});
3. Front-end - Páginas
1. pages/tarefa/listar
Lista todas as tarefas em uma tabela:

tsx
Copiar código
import React, { useEffect, useState } from "react";
import axios from "axios";

const ListarTarefas = () => {
    const [tarefas, setTarefas] = useState([]);

    useEffect(() => {
        axios.get("/api/tarefa/listar").then((res) => setTarefas(res.data));
    }, []);

    return (
        <table>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Nome</th>
                    <th>Categoria</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                {tarefas.map((tarefa) => (
                    <tr key={tarefa.id}>
                        <td>{tarefa.id}</td>
                        <td>{tarefa.nome}</td>
                        <td>{tarefa.categoria}</td>
                        <td>{tarefa.status}</td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
};

export default ListarTarefas;
2. pages/tarefa/cadastrar
Cria uma nova tarefa:

tsx
Copiar código
import React, { useState } from "react";
import axios from "axios";

const CadastrarTarefa = () => {
    const [nome, setNome] = useState("");
    const [descricao, setDescricao] = useState("");
    const [categoria, setCategoria] = useState("");

    const handleSubmit = () => {
        axios.post("/api/tarefa/cadastrar", { nome, descricao, categoria })
            .then(() => alert("Tarefa cadastrada com sucesso!"));
    };

    return (
        <form onSubmit={(e) => { e.preventDefault(); handleSubmit(); }}>
            <input type="text" placeholder="Nome" value={nome} onChange={(e) => setNome(e.target.value)} />
            <textarea placeholder="Descrição" value={descricao} onChange={(e) => setDescricao(e.target.value)} />
            <input type="text" placeholder="Categoria" value={categoria} onChange={(e) => setCategoria(e.target.value)} />
            <button type="submit">Cadastrar</button>
        </form>
    );
};

export default CadastrarTarefa;
3. pages/tarefa/alterar
Altera o status da tarefa:

tsx
Copiar código
import React, { useState } from "react";
import axios from "axios";

const AlterarTarefa = ({ id }) => {
    const [status, setStatus] = useState("");

    const handleAlterar = () => {
        axios.patch(`/api/tarefa/alterar/${id}`)
            .then((res) => setStatus(res.data.status));
    };

    return (
        <div>
            <button onClick={handleAlterar}>Alterar Status</button>
            <p>Status Atual: {status}</p>
        </div>
    );
};

export default AlterarTarefa;
4. pages/tarefa/concluidas
Lista tarefas concluídas:

tsx
Copiar código
// Similar à página listar, alterando o endpoint para `/api/tarefa/concluidas`
5. pages/tarefa/naoconcluidas
Lista tarefas não concluídas:

tsx
Copiar código
// Similar à página listar, alterando o endpoint para `/api/tarefa/naoconcluidas`
4. Finalização
Crie o arquivo de testes (.http) com exemplos de requisições para todos os endpoints.
    */
}

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

    [HttpGet]
    [Route("naoconcluidas")]
    public IActionResult ListarNaoConcluidas()
    {
        try
        {
            List<Tarefa> tarefasNaoConcluidas = _context.Tarefas
                .Include(x => x.Categoria)
                .Where(x => x.Status != "Concluída")
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
                .Where(x => x.Status == "Concluída")
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
        else if (tarefa.Status == "Em andamento")
        {
            tarefa.Status = "Concluída";
        }

        _context.Tarefas.Update(tarefa);
        _context.SaveChanges();
        return Ok(tarefa);
    }
}

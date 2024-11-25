import React from 'react';
import { BrowserRouter, Link, Route, Routes } from 'react-router-dom';
import TarefaCadastrar from './pages/tarefas/tarefa-cadastrar';
import TarefaListar from './pages/tarefas/tarefa-listar';
import TarefaListarConcluida from './pages/tarefas/tarefa-concluida';
import TarefaListarNaoConcluida from './pages/tarefas/tarefa-nao-concluida';
import TarefaAlterar from './pages/tarefas/tarefa-alterar';

function App() {
  return (
    <div className="App">
      <BrowserRouter>
        <nav>
          <ul>
            <li>
              <Link to="/">Home</Link>
            </li>
            <li>
              <Link to="/tarefa/cadastrar">Cadastrar Tarefa</Link>
            </li>
            <li>
              <Link to="/tarefa/listar">Listar Tarefas</Link>
            </li>
            <li>
              <Link to="/tarefa/listar/concluida">Listar Tarefas Concluidas</Link>
            </li>
            <li>
              <Link to="/tarefa/listar/nao-concluida">Listar Tarefas Não Concluidas</Link>
            </li>
          </ul>
        </nav>
        <Routes>
          <Route path="/" element={<TarefaListar/>}></Route>
          <Route path="/tarefa/cadastrar" element={<TarefaCadastrar/>}></Route>
          <Route path="/tarefa/listar" element={<TarefaListar/>}></Route>
          <Route path="/tarefa/listar/concluida" element={<TarefaListarConcluida/>}></Route>
          <Route path="/tarefa/listar/nao-concluida" element={<TarefaListarNaoConcluida/>}></Route>
          <Route path={"/tarefa/alterar/:tarefaId" }element={<TarefaAlterar/>}></Route>
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;

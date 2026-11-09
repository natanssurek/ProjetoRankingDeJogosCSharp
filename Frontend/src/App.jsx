import { useState, useEffect } from 'react'
import './App.css'

const API = '/api'

export default function App() {
  const [jogos, setJogos] = useState([])
  const [ranking, setRanking] = useState([])
  const [busca, setBusca] = useState('')
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')
  const [detalhe, setDetalhe] = useState(null)

  const [form, setForm] = useState({ titulo: '', anoLancamento: 2024, genero: '' })
  const [avaliacao, setAvaliacao] = useState({ autor: '', nota: 8, comentario: '' })

  async function carregarJogos(termo = '') {
    setCarregando(true)
    setErro('')
    try {
      const url = termo ? `${API}/jogos?busca=${encodeURIComponent(termo)}` : `${API}/jogos`
      const res = await fetch(url)
      if (!res.ok) throw new Error('Falha ao carregar jogos')
      setJogos(await res.json())
    } catch (e) {
      setErro(e.message)
    } finally {
      setCarregando(false)
    }
  }

  async function carregarRanking() {
    try {
      const res = await fetch(`${API}/ranking`)
      if (!res.ok) throw new Error('Falha ao carregar ranking')
      setRanking(await res.json())
    } catch (e) {
      setErro(e.message)
    }
  }

  useEffect(() => {
    carregarJogos()
    carregarRanking()
  }, [])

  async function abrirDetalhe(id) {
    setErro('')
    try {
      const res = await fetch(`${API}/jogos/${id}`)
      if (!res.ok) throw new Error('Jogo não encontrado')
      setDetalhe(await res.json())
    } catch (e) {
      setErro(e.message)
    }
  }

  async function cadastrar(e) {
    e.preventDefault()
    setErro('')
    try {
      const res = await fetch(`${API}/jogos`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          titulo: form.titulo,
          anoLancamento: Number(form.anoLancamento),
          genero: form.genero,
        }),
      })
      const data = await res.json()
      if (!res.ok) throw new Error(data.erro || 'Erro ao cadastrar')
      setForm({ titulo: '', anoLancamento: 2024, genero: '' })
      carregarJogos()
      carregarRanking()
    } catch (e) {
      setErro(e.message)
    }
  }

  async function remover(id) {
    if (!confirm('Remover este jogo?')) return
    setErro('')
    try {
      const res = await fetch(`${API}/jogos/${id}`, { method: 'DELETE' })
      if (!res.ok && res.status !== 204) throw new Error('Erro ao remover')
      if (detalhe?.id === id) setDetalhe(null)
      carregarJogos()
      carregarRanking()
    } catch (e) {
      setErro(e.message)
    }
  }

  async function avaliar(e) {
    e.preventDefault()
    if (!detalhe) return
    setErro('')
    try {
      const res = await fetch(`${API}/jogos/${detalhe.id}/avaliacoes`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          autor: avaliacao.autor,
          nota: Number(avaliacao.nota),
          comentario: avaliacao.comentario,
        }),
      })
      const data = await res.json()
      if (!res.ok) throw new Error(data.erro || 'Erro ao avaliar')
      setAvaliacao({ autor: '', nota: 8, comentario: '' })
      abrirDetalhe(detalhe.id)
      carregarRanking()
    } catch (e) {
      setErro(e.message)
    }
  }

  return (
    <main className="app">
      <h1>Catálogo de Jogos</h1>

      {erro && <p className="erro">{erro}</p>}

      <section>
        <h2>Buscar</h2>
        <form onSubmit={(e) => { e.preventDefault(); carregarJogos(busca) }}>
          <input
            value={busca}
            onChange={(e) => setBusca(e.target.value)}
            placeholder="Título do jogo"
          />
          <button type="submit">Buscar</button>
          <button type="button" onClick={() => { setBusca(''); carregarJogos('') }}>
            Limpar
          </button>
        </form>
      </section>

      <section>
        <h2>Cadastrar jogo</h2>
        <form onSubmit={cadastrar}>
          <input
            value={form.titulo}
            onChange={(e) => setForm({ ...form, titulo: e.target.value })}
            placeholder="Título"
            required
          />
          <input
            type="number"
            value={form.anoLancamento}
            onChange={(e) => setForm({ ...form, anoLancamento: e.target.value })}
            placeholder="Ano"
            min="1900"
            max="2100"
            required
          />
          <input
            value={form.genero}
            onChange={(e) => setForm({ ...form, genero: e.target.value })}
            placeholder="Gênero"
            required
          />
          <button type="submit">Cadastrar</button>
        </form>
      </section>

      <section>
        <h2>Jogos {carregando && '(carregando...)'}</h2>
        {jogos.length === 0 && !carregando && <p>Nenhum jogo cadastrado.</p>}
        <ul className="lista">
          {jogos.map((j) => (
            <li key={j.id}>
              <span>
                <strong>{j.titulo}</strong> ({j.anoLancamento}) — {j.genero}
              </span>
              <span className="acoes">
                <button onClick={() => abrirDetalhe(j.id)}>Detalhes</button>
                <button onClick={() => remover(j.id)}>Remover</button>
              </span>
            </li>
          ))}
        </ul>
      </section>

      {detalhe && (
        <section className="detalhe">
          <h2>{detalhe.titulo}</h2>
          <p>
            Ano: {detalhe.anoLancamento} | Gênero: {detalhe.genero} | Média:{' '}
            <strong>{detalhe.mediaAvaliacao}</strong>
          </p>

          <h3>Avaliações ({detalhe.avaliacoes.length})</h3>
          {detalhe.avaliacoes.length === 0 && <p>Sem avaliações ainda.</p>}
          <ul>
            {detalhe.avaliacoes.map((a, i) => (
              <li key={i}>
                <strong>{a.autor}</strong> — nota {a.nota}
                {a.comentario && ` — "${a.comentario}"`}
              </li>
            ))}
          </ul>

          <h3>Avaliar</h3>
          <form onSubmit={avaliar}>
            <input
              value={avaliacao.autor}
              onChange={(e) => setAvaliacao({ ...avaliacao, autor: e.target.value })}
              placeholder="Seu nome"
              required
            />
            <input
              type="number"
              value={avaliacao.nota}
              onChange={(e) => setAvaliacao({ ...avaliacao, nota: e.target.value })}
              min="0"
              max="10"
              required
            />
            <input
              value={avaliacao.comentario}
              onChange={(e) => setAvaliacao({ ...avaliacao, comentario: e.target.value })}
              placeholder="Comentário (opcional)"
            />
            <button type="submit">Enviar avaliação</button>
          </form>

          <button onClick={() => setDetalhe(null)}>Fechar</button>
        </section>
      )}

      <section>
        <h2>Ranking</h2>
        {ranking.length === 0 && <p>Sem dados de ranking.</p>}
        <ol>
          {ranking.map((r) => (
            <li key={r.id}>
              <strong>{r.titulo}</strong> ({r.anoLancamento}) — {r.genero} — média{' '}
              <strong>{r.mediaAvaliacao}</strong>
            </li>
          ))}
        </ol>
      </section>
    </main>
  )
}

export class ApiError extends Error {
  constructor(status, detail) {
    super(detail || `HTTP ${status}`)
    this.status = status
  }
}

async function request(url, options = {}) {
  const response = await fetch(url, options)
  if (!response.ok) {
    let detail = ''
    try {
      const problem = await response.json()
      detail = problem.detail || problem.title || ''
      if (problem.errors) detail = Object.values(problem.errors).flat().join(' ')
    } catch {
      /* body is not ProblemDetails */
    }
    throw new ApiError(response.status, detail)
  }
  return response
}

const json = (body) => ({
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(body)
})

export async function startGame(game, language) {
  return (await request('/api/games', json({ game, language }))).json()
}

export async function getGame(id) {
  return (await request(`/api/games/${id}`)).json()
}

export async function endGame(id) {
  await request(`/api/games/${id}`, { method: 'DELETE' })
}

export async function sendCommand(id, command) {
  return (await request(`/api/games/${id}/commands`, json({ command }))).json()
}

export async function downloadTrack(id) {
  const response = await request(`/api/games/${id}/track`)
  const disposition = response.headers.get('Content-Disposition') || ''
  const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition)
  return { blob: await response.blob(), fileName: match ? match[1] : 'FORTALEZA.TRK' }
}

export async function uploadTrack(id, file) {
  const body = await file.arrayBuffer()
  const init = { method: 'PUT', headers: { 'Content-Type': 'application/octet-stream' }, body }
  return (await request(`/api/games/${id}/track`, init)).json()
}

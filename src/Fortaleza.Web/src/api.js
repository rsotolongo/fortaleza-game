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
  return response.json()
}

const post = (body) => ({
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify(body)
})

export const startGame = (game, language) => request('/api/games/start', post({ game, language }))

/** The server keeps no state: the track (commands so far) travels with every command. */
export const play = (game, language, track, command) =>
  request('/api/games/play', post({ game, language, track, command }))

export async function loadTrack(game, language, file) {
  const body = await file.arrayBuffer()
  const init = { method: 'PUT', headers: { 'Content-Type': 'application/octet-stream' }, body }
  return request(`/api/games/load?game=${game}&language=${language}`, init)
}

/** Track file text: accents stripped and CRLF endings, so the original Pascal games can run it too. */
export function trackText(track) {
  return track.map((line) => line.normalize('NFD').replace(/\p{M}/gu, '') + '\r\n').join('')
}

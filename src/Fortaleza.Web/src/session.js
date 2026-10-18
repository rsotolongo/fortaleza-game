const KEY = 'fortaleza.session'
const MAX_ENTRIES = 400

export function loadSession() {
  try {
    const raw = localStorage.getItem(KEY)
    return raw ? JSON.parse(raw) : null
  } catch {
    return null
  }
}

export function saveSession(session) {
  try {
    const entries = session.entries.slice(-MAX_ENTRIES)
    localStorage.setItem(KEY, JSON.stringify({ ...session, entries }))
  } catch {
    /* storage unavailable: the game still works, it just cannot be resumed */
  }
}

export function clearSession() {
  try {
    localStorage.removeItem(KEY)
  } catch {
    /* ignore */
  }
}

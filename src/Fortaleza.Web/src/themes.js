const KEY = 'fortaleza.theme'

const DEFAULT = 'dungeon'

const mono = "ui-monospace, 'Cascadia Mono', Consolas, 'Courier New', monospace"

export const themes = {
  dungeon: {
    names: { es: 'Mazmorra', en: 'Dungeon' },
    vars: {
      '--bg': '#14110f',
      '--surface': '#1f1a17',
      '--surface-2': '#2a231f',
      '--border': '#3a302a',
      '--text': '#e9e2d6',
      '--muted': '#a39887',
      '--accent': '#d9a441',
      '--accent-text': '#1a1408',
      '--echo': '#8fc1a9',
      '--danger': '#e0715f',
      '--good': '#7fcf8a',
      'color-scheme': 'dark'
    }
  },
  parchment: {
    names: { es: 'Pergamino', en: 'Parchment' },
    vars: {
      '--bg': '#f5efe4',
      '--surface': '#fffaf0',
      '--surface-2': '#efe6d4',
      '--border': '#d8ccb4',
      '--text': '#2a2118',
      '--muted': '#7a6c58',
      '--accent': '#9a6a12',
      '--accent-text': '#fff8e8',
      '--echo': '#2f7358',
      '--danger': '#b3422f',
      '--good': '#2f7d3d',
      'color-scheme': 'light'
    }
  },
  terminal: {
    names: { es: 'Terminal', en: 'Terminal' },
    vars: {
      '--bg': '#000000',
      '--surface': '#071007',
      '--surface-2': '#0d1d0d',
      '--border': '#1c4a1c',
      '--text': '#5dff6a',
      '--muted': '#33b341',
      '--accent': '#5dff6a',
      '--accent-text': '#000000',
      '--echo': '#ffd24a',
      '--danger': '#ff5f56',
      '--good': '#5dff6a',
      '--narrative': mono,
      '--ui': mono,
      'color-scheme': 'dark'
    }
  },
  contrast: {
    names: { es: 'Nítido', en: 'Stark' },
    vars: {
      '--bg': '#000000',
      '--surface': '#0a0a0a',
      '--surface-2': '#1a1a1a',
      '--border': '#ffffff',
      '--text': '#ffffff',
      '--muted': '#d0d0d0',
      '--accent': '#ffe600',
      '--accent-text': '#000000',
      '--echo': '#7ff0ff',
      '--danger': '#ff7a7a',
      '--good': '#7aff9a',
      'color-scheme': 'dark'
    }
  },
  daylight: {
    names: { es: 'Diurno', en: 'Daylight' },
    vars: {
      '--bg': '#fbfaf6',
      '--surface': '#ffffff',
      '--surface-2': '#ebe9e2',
      '--border': '#1a1a1a',
      '--text': '#0a0a0a',
      '--muted': '#262626',
      '--accent': '#0a2fb8',
      '--accent-text': '#ffffff',
      '--echo': '#004d26',
      '--danger': '#a80000',
      '--good': '#005f1c',
      '--weight': '500',
      'color-scheme': 'light'
    }
  }
}

const managed = [...new Set(Object.values(themes).flatMap((theme) => Object.keys(theme.vars)))]

export function loadTheme() {
  try {
    const stored = localStorage.getItem(KEY)
    return stored in themes ? stored : DEFAULT
  } catch {
    return DEFAULT
  }
}

export function applyTheme(id) {
  const root = document.documentElement
  for (const name of managed) root.style.removeProperty(name)
  const theme = themes[id]
  if (theme) for (const [name, value] of Object.entries(theme.vars)) root.style.setProperty(name, value)
  const bg = getComputedStyle(root).getPropertyValue('--bg').trim()
  document.querySelector('meta[name="theme-color"]')?.setAttribute('content', bg)
}

export function saveTheme(id) {
  try {
    localStorage.setItem(KEY, id)
  } catch {
    /* storage unavailable: the theme still applies for this visit */
  }
}

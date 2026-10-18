const stripWeight = (line) => line.replace(/\s*\(\d+\)\s*$/, '')

/** Turns the plain lines returned by the API into transcript entries (text blocks and tappable object lists). */
export function toEntries(lines, markers) {
  const entries = []
  let items = null
  let weighted = false

  const flush = () => {
    if (items && items.length) entries.push({ kind: 'items', items })
    items = null
  }

  for (const line of lines) {
    if (line === markers.room || line === markers.bag) {
      flush()
      entries.push({ kind: 'text', text: line, emphasis: true })
      items = []
      weighted = line === markers.bag
    } else if (items && line.trim() !== '' && (!weighted || /\(\d+\)\s*$/.test(line))) {
      items.push({ label: line, name: stripWeight(line) })
    } else {
      flush()
      entries.push({ kind: 'text', text: line })
    }
  }
  flush()
  return entries
}

import { ref } from 'vue'

const KEY = 'fortaleza.interaction'

export const SCROLL = 'scroll'
export const TYPEWRITE = 'typewrite'

function load() {
  try {
    return localStorage.getItem(KEY) === TYPEWRITE ? TYPEWRITE : SCROLL
  } catch {
    return SCROLL
  }
}

export const interaction = ref(load())

export function setInteraction(mode) {
  interaction.value = mode
  try {
    localStorage.setItem(KEY, mode)
  } catch {
    /* storage unavailable: the mode still applies for this visit */
  }
}

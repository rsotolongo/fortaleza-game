<script setup>
import { ref } from 'vue'
import { texts } from '../i18n'
import { applyTheme, loadTheme, saveTheme, themes } from '../themes'

defineProps({ language: String })

const dialog = ref(null)
const theme = ref(loadTheme())
const open = ref(false)

function choose(id) {
  theme.value = id
  open.value = false
  applyTheme(id)
  saveTheme(id)
}

function step(delta) {
  const ids = Object.keys(themes)
  choose(ids[(ids.indexOf(theme.value) + delta + ids.length) % ids.length])
}

function leave(event) {
  if (!event.currentTarget.contains(event.relatedTarget)) open.value = false
}

function escape(event) {
  if (!open.value) return
  event.stopPropagation()
  event.preventDefault()
  open.value = false
}

function show() {
  if (!dialog.value.open) dialog.value.showModal()
}

function backdrop(event) {
  if (event.target === dialog.value) dialog.value.close()
}

defineExpose({ show })
</script>

<template>
  <dialog ref="dialog" class="about settings" aria-labelledby="settings-title" @click="backdrop">
    <div class="about-body">
      <header>
        <div class="crest" aria-hidden="true">⚙️</div>
        <h2 id="settings-title">{{ texts[language].settings }}</h2>
      </header>

      <div class="field">
        <span id="theme-label">{{ texts[language].theme }}</span>
        <div class="select" @focusout="leave">
          <button
            type="button"
            class="trigger"
            aria-haspopup="listbox"
            aria-labelledby="theme-label"
            :aria-expanded="open"
            @click="open = !open"
            @keydown.down.prevent="step(1)"
            @keydown.up.prevent="step(-1)"
            @keydown.esc="escape"
          >
            <span>{{ themes[theme].names[language] }}</span>
            <svg viewBox="0 0 12 8" width="12" height="8" aria-hidden="true">
              <path d="M1 1.5l5 5 5-5" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
          </button>
          <ul v-if="open" role="listbox" aria-labelledby="theme-label" @mousedown.prevent @keydown.esc="escape">
            <li v-for="(entry, id) in themes" :key="id">
              <button type="button" role="option" :aria-selected="id === theme" @click="choose(id)">
                {{ entry.names[language] }}
                <span v-if="id === theme" aria-hidden="true">✓</span>
              </button>
            </li>
          </ul>
        </div>
      </div>

      <form method="dialog">
        <button class="primary" autofocus>{{ texts[language].close }}</button>
      </form>
    </div>
  </dialog>
</template>

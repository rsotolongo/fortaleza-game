<script setup>
import { nextTick, ref, watch } from 'vue'
import { texts } from '../i18n'

const props = defineProps({
  language: String,
  game: Number,
  status: String,
  entries: Array,
  busy: Boolean
})
const emit = defineEmits(['command', 'save', 'load', 'new'])

const input = ref('')
const field = ref(null)
const scroller = ref(null)
const fileInput = ref(null)
const menuOpen = ref(false)
const history = []
let cursor = -1

const ended = () => props.status !== 'playing'

watch(
  () => props.entries.length,
  async () => {
    await nextTick()
    if (scroller.value) scroller.value.scrollTop = scroller.value.scrollHeight
  },
  { immediate: true, flush: 'post' }
)

function submit() {
  const command = input.value.trim()
  if (!command || props.busy || ended()) return
  history.push(command)
  cursor = -1
  input.value = ''
  emit('command', command)
}

function recall(step) {
  if (!history.length) return
  cursor = cursor === -1 ? history.length - 1 : Math.min(history.length - 1, Math.max(0, cursor + step))
  input.value = history[cursor]
}

function verb(entry) {
  input.value = entry[1]
  if (entry[1].endsWith(' ')) field.value?.focus()
  else submit()
}

function useItem(name) {
  const current = input.value
  const separator = current && !current.endsWith(' ') ? ' ' : ''
  input.value = `${current}${separator}${name.toLowerCase()}`
  field.value?.focus()
}

function pickFile(event) {
  const file = event.target.files?.[0]
  event.target.value = ''
  menuOpen.value = false
  if (file) emit('load', file)
}
</script>

<template>
  <div class="game">
    <header class="bar">
      <h1>
        {{ texts[language].title }} <small>{{ game === 1 ? 'I' : 'II' }}</small>
      </h1>
      <div class="menu">
        <button class="icon" :aria-label="texts[language].menu" :aria-expanded="menuOpen" @click="menuOpen = !menuOpen">⋯</button>
        <div v-if="menuOpen" class="sheet" @click="menuOpen = false">
          <button @click="emit('save')">⬇ {{ texts[language].save }}</button>
          <button @click.stop="fileInput.click()">⬆ {{ texts[language].load }}</button>
          <button @click="emit('new')">✦ {{ texts[language].newGame }}</button>
        </div>
        <input ref="fileInput" type="file" accept=".trk,.TRK,text/plain" hidden @change="pickFile" />
      </div>
    </header>

    <div ref="scroller" class="transcript" aria-live="polite">
      <template v-for="(entry, index) in entries" :key="index">
        <p v-if="entry.kind === 'input'" class="echo">› {{ entry.text }}</p>
        <p v-else-if="entry.kind === 'text'" :class="{ emphasis: entry.emphasis, blank: entry.text === '' }">{{ entry.text }}</p>
        <ul v-else class="items" :aria-label="texts[language].objects">
          <li v-for="item in entry.items" :key="item.label">
            <button :disabled="ended()" @click="useItem(item.name)">{{ item.label }}</button>
          </li>
        </ul>
      </template>

      <div v-if="status !== 'playing'" class="banner" :class="status">
        <strong>{{ status === 'won' ? texts[language].won : texts[language].lost }}</strong>
        <button @click="emit('new')">{{ texts[language].playAgain }}</button>
      </div>
    </div>

    <footer class="console">
      <nav class="verbs" aria-label="verbs">
        <button v-for="entry in texts[language].verbs" :key="entry[0]" :disabled="busy || ended()" @click="verb(entry)">
          {{ entry[0] }}
        </button>
      </nav>
      <form class="line" @submit.prevent="submit">
        <input
          ref="field"
          v-model="input"
          type="text"
          enterkeyhint="send"
          autocomplete="off"
          autocapitalize="off"
          autocorrect="off"
          spellcheck="false"
          maxlength="200"
          :placeholder="texts[language].placeholder"
          :disabled="ended()"
          @keydown.up.prevent="recall(-1)"
          @keydown.down.prevent="recall(1)"
        />
        <button type="submit" :disabled="busy || ended() || !input.trim()" :aria-label="texts[language].send">➤</button>
      </form>
    </footer>
  </div>
</template>

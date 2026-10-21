<script setup>
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import { texts } from '../i18n'
import { TYPEWRITE, interaction } from '../interaction'

const props = defineProps({
  language: String,
  game: Number,
  status: String,
  entries: Array,
  busy: Boolean,
  settled: Boolean
})
const emit = defineEmits(['command', 'save', 'load', 'new', 'about', 'introduction', 'advices', 'settings', 'sheet'])

const input = ref('')
const field = ref(null)
const scroller = ref(null)
const fileInput = ref(null)
const menuOpen = ref(false)
const history = []
let cursor = -1

const ended = () => props.status !== 'playing'

const TYPE_DELAY = 14
const TEAR_HOLD = 900
const TEAR_FEED = 1300
const PAPER_EDGE = '█▄▄▄▄██▄▄▄▄▄▄██▄▄  ▄▄█████▄▄▄▄'

const typewrite = computed(() => interaction.value === TYPEWRITE)
const done = ref(0)
const sheetStart = ref(0)
const sheet = ref(0)
const typing = ref(null)
const tearing = ref(false)
const topEdge = ref('')
const bottomEdge = ref('')
let epoch = 0
let running = false

const prefersReducedMotion = () => window.matchMedia('(prefers-reduced-motion: reduce)').matches
const sleep = (ms) => new Promise((resolve) => setTimeout(resolve, ms))

function edge(char) {
  const offset = Math.floor(Math.random() * PAPER_EDGE.length)
  const row = PAPER_EDGE.slice(offset) + PAPER_EDGE.slice(0, offset)
  return row.repeat(Math.ceil(160 / row.length)).replaceAll('▄', char)
}

function lastTear(upTo) {
  for (let i = upTo - 1; i >= 0; i--) if (props.entries[i].kind === 'tear') return i
  return -1
}

function newSheet(start) {
  sheetStart.value = start
  sheet.value++
  topEdge.value = edge('▄')
}

function settle() {
  epoch++
  typing.value = null
  tearing.value = false
  done.value = props.entries.length
  const start = lastTear(done.value) + 1
  if (start !== sheetStart.value) newSheet(start)
}

async function typeOut(index, text, mine) {
  for (let length = 1; length <= text.length; length++) {
    if (mine !== epoch) return
    typing.value = { index, length }
    await sleep(TYPE_DELAY)
  }
  typing.value = null
}

async function tearOff(mine) {
  bottomEdge.value = edge('▀')
  tearing.value = true
  await sleep(TEAR_HOLD)
  if (mine !== epoch) return
  newSheet(done.value + 1)
  tearing.value = false
  await sleep(TEAR_FEED)
}

async function play() {
  if (running) return
  running = true
  const mine = epoch
  if (prefersReducedMotion()) settle()
  while (typewrite.value && mine === epoch && done.value < props.entries.length) {
    const entry = props.entries[done.value]
    if (entry.kind === 'tear') await tearOff(mine)
    else if (entry.kind === 'text' && entry.text.trim() !== '') await typeOut(done.value, entry.text, mine)
    if (mine !== epoch) break
    done.value++
  }
  running = false
  if (typewrite.value && done.value < props.entries.length) play()
}

function skipTyping() {
  if (typewrite.value && (typing.value || tearing.value || done.value < props.entries.length)) settle()
}

const settledNow = computed(() => !typewrite.value || (done.value >= props.entries.length && !typing.value && !tearing.value))

const visible = computed(() => {
  const list = []
  const end = typewrite.value ? done.value : props.entries.length
  for (let index = typewrite.value ? sheetStart.value : 0; index < end; index++) {
    if (props.entries[index].kind !== 'tear') list.push({ entry: props.entries[index], index })
  }
  if (typewrite.value && typing.value) {
    list.push({ entry: props.entries[typing.value.index], index: typing.value.index, length: typing.value.length })
  }
  return list
})

const shownText = (item) => (item.length === undefined ? item.entry.text : item.entry.text.slice(0, item.length))

watch(
  () => props.entries.length,
  (length, previous) => {
    if (length < previous || length < done.value) {
      epoch++
      running = false
      typing.value = null
      tearing.value = false
      done.value = 0
      newSheet(0)
    }
    if (typewrite.value) play()
  }
)

watch(typewrite, () => {
  settle()
})

watch(
  () => [visible.value.length, typing.value?.length, tearing.value, ended()],
  async () => {
    await nextTick()
    if (scroller.value) scroller.value.scrollTop = scroller.value.scrollHeight
  },
  { immediate: true, flush: 'post' }
)

onMounted(() => {
  topEdge.value = edge('▄')
  if (props.settled) settle()
  else if (typewrite.value) play()
})

onBeforeUnmount(() => {
  epoch++
})

function submit() {
  const command = input.value.trim()
  if (!command || props.busy || ended()) return
  history.push(command)
  cursor = -1
  input.value = ''
  skipTyping()
  emit('command', command)
}

function freshSheet(event) {
  if (!typewrite.value || props.busy || ended()) return
  event.preventDefault()
  input.value = ''
  skipTyping()
  emit('sheet')
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
  <main class="game">
    <header class="bar">
      <h1 tabindex="-1">
        {{ texts[language].title }} <small>{{ game === 1 ? 'I' : 'II' }}</small>
      </h1>
      <div class="menu" @keydown.esc="menuOpen = false">
        <button class="icon" :aria-label="texts[language].menu" aria-haspopup="true" :aria-expanded="menuOpen" @click="menuOpen = !menuOpen">⋯</button>
        <div v-if="menuOpen" class="sheet" @click="menuOpen = false">
          <button @click="emit('save')"><span aria-hidden="true">⬇</span> {{ texts[language].save }}</button>
          <button @click.stop="fileInput.click()"><span aria-hidden="true">⬆</span> {{ texts[language].load }}</button>
          <button @click="emit('new')"><span aria-hidden="true">✦</span> {{ texts[language].newGame }}</button>
          <button @click="emit('introduction')"><span aria-hidden="true">📜</span> {{ texts[language].introduction }}</button>
          <button @click="emit('advices')"><span aria-hidden="true">💡</span> {{ texts[language].advices }}</button>
          <button @click="emit('settings')"><span aria-hidden="true">⚙️</span> {{ texts[language].settings }}</button>
          <button @click="emit('about')"><span aria-hidden="true">ⓘ</span> {{ texts[language].about }}</button>
        </div>
        <input ref="fileInput" type="file" accept=".trk,.TRK,text/plain" hidden @change="pickFile" />
      </div>
    </header>

    <div ref="scroller" class="transcript" :class="{ typewrite }" role="log" tabindex="0" :aria-label="texts[language].transcript" @pointerdown="skipTyping">
      <Transition :name="typewrite ? 'feed' : 'none'" mode="out-in">
        <div :key="sheet" class="page" :class="{ paper: typewrite, tearing: typewrite && tearing }">
          <div v-if="typewrite" class="edge top" aria-hidden="true">{{ topEdge }}</div>
          <template v-for="item in visible" :key="item.index">
            <p v-if="item.entry.kind === 'input'" class="echo">› {{ item.entry.text }}</p>
            <p
              v-else-if="item.entry.kind === 'text'"
              :class="{ emphasis: item.entry.emphasis, blank: item.entry.text === '', typing: item.length !== undefined }"
            >
              <template v-if="typewrite">
                <span aria-hidden="true">{{ shownText(item) }}</span><span class="sr-only">{{ item.entry.text }}</span>
              </template>
              <template v-else>{{ item.entry.text }}</template>
            </p>
            <ul v-else class="items" :aria-label="texts[language].objects">
              <li v-for="object in item.entry.items" :key="object.label">
                <button :disabled="ended()" @click="useItem(object.name)">{{ object.label }}</button>
              </li>
            </ul>
          </template>

          <div v-if="status !== 'playing' && settledNow" class="banner" :class="status">
            <strong>{{ status === 'won' ? texts[language].won : texts[language].lost }}</strong>
            <button @click="emit('new')">{{ texts[language].playAgain }}</button>
          </div>
          <div v-if="typewrite && tearing" class="edge bottom" aria-hidden="true">{{ bottomEdge }}</div>
        </div>
      </Transition>
    </div>

    <footer class="console">
      <nav class="verbs" :aria-label="texts[language].verbsLabel" :tabindex="ended() ? 0 : undefined">
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
          :aria-label="texts[language].commandLabel"
          autocapitalize="off"
          autocorrect="off"
          spellcheck="false"
          maxlength="200"
          :placeholder="texts[language].placeholder"
          :disabled="ended()"
          @keydown.up.prevent="recall(-1)"
          @keydown.down.prevent="recall(1)"
          @keydown.esc="skipTyping"
          @keydown.ctrl.l="freshSheet"
        />
        <button type="submit" :disabled="busy || ended() || !input.trim()" :aria-label="texts[language].send">➤</button>
      </form>
    </footer>
  </main>
</template>

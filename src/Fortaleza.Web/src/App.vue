<script setup>
import { computed, ref } from 'vue'
import * as api from './api'
import { browserLanguage, texts } from './i18n'
import { clearSession, loadSession, saveSession } from './session'
import { toEntries } from './transcript'
import StartScreen from './components/StartScreen.vue'
import GameView from './components/GameView.vue'
import AboutDialog from './components/AboutDialog.vue'
import ReaderDialog from './components/ReaderDialog.vue'

const stored = ref(loadSession())
const uiLanguage = ref(stored.value?.language ?? browserLanguage())
const session = ref(null)
const entries = ref([])
const busy = ref(false)
const notice = ref('')
const about = ref(null)
const reader = ref(null)
let noticeTimer = 0

const gameLanguage = computed(() => session.value?.language ?? uiLanguage.value)

function toast(message) {
  notice.value = message
  clearTimeout(noticeTimer)
  noticeTimer = setTimeout(() => (notice.value = ''), 3500)
}

function persist() {
  if (session.value) saveSession({ ...session.value, entries: entries.value })
}

function apply(response, echo) {
  session.value = {
    game: response.game,
    language: response.language,
    status: response.status,
    track: response.track
  }
  if (echo) entries.value.push({ kind: 'input', text: echo })
  entries.value.push(...toEntries(response.output, texts[response.language].markers))
  persist()
}

function reset() {
  clearSession()
  stored.value = null
  session.value = null
  entries.value = []
}

async function guarded(action) {
  busy.value = true
  try {
    await action()
  } catch (error) {
    toast(error.message || texts[gameLanguage.value].error)
  } finally {
    busy.value = false
  }
}

const start = (game) =>
  guarded(async () => {
    entries.value = []
    apply(await api.startGame(game, uiLanguage.value))
  })

function resume() {
  const saved = stored.value
  uiLanguage.value = saved.language
  session.value = { game: saved.game, language: saved.language, status: saved.status, track: saved.track }
  entries.value = saved.entries
}

const command = (text) =>
  guarded(async () => {
    const { game, language, track } = session.value
    apply(await api.play(game, language, track, text), text)
  })

function save() {
  const track = session.value.track
  if (!track.length) {
    toast(texts[gameLanguage.value].nothingToSave)
    return
  }
  const { game, language } = session.value
  const link = document.createElement('a')
  link.href = URL.createObjectURL(new Blob([api.trackText(track)], { type: "text/plain" }))
  link.download = `FORT${game}_${language.toUpperCase()}.TRK`
  link.click()
  URL.revokeObjectURL(link.href)
  toast(texts[gameLanguage.value].saved)
}

const load = (file) =>
  guarded(async () => {
    const response = await api.loadTrack(session.value.game, session.value.language, file)
    entries.value = []
    apply(response)
    toast(texts[gameLanguage.value].loaded)
  })
</script>

<template>
  <StartScreen
    v-if="!session"
    :language="uiLanguage"
    :resumable="stored"
    :busy="busy"
    @language="uiLanguage = $event"
    @start="start"
    @resume="resume"
    @about="about.show()"
    @introduction="reader.show('introduction', $event)"
    @advices="reader.show('advices')"
  />
  <GameView
    v-else
    :language="session.language"
    :game="session.game"
    :status="session.status"
    :entries="entries"
    :busy="busy"
    @command="command"
    @save="save"
    @load="load"
    @new="reset"
    @about="about.show()"
    @introduction="reader.show('introduction', session.game)"
    @advices="reader.show('advices')"
  />
  <AboutDialog ref="about" :language="gameLanguage" />
  <ReaderDialog ref="reader" :language="gameLanguage" />
  <transition name="fade">
    <div v-if="notice" class="toast" role="status">{{ notice }}</div>
  </transition>
</template>

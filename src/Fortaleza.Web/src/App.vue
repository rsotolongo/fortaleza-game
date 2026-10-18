<script setup>
import { computed, ref } from 'vue'
import * as api from './api'
import { browserLanguage, texts } from './i18n'
import { clearSession, loadSession, saveSession } from './session'
import { toEntries } from './transcript'
import StartScreen from './components/StartScreen.vue'
import GameView from './components/GameView.vue'

const stored = ref(loadSession())
const uiLanguage = ref(stored.value?.language ?? browserLanguage())
const session = ref(null)
const entries = ref([])
const busy = ref(false)
const notice = ref('')
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
  session.value = { id: response.id, game: response.game, language: response.language, status: response.status }
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
    const language = gameLanguage.value
    if (error.status === 404) {
      toast(texts[language].expired)
      reset()
    } else if (error.status === 409) {
      toast(texts[language].nothingToSave)
    } else {
      toast(error.message || texts[language].error)
    }
  } finally {
    busy.value = false
  }
}

function newGame() {
  const id = session.value?.id
  if (id) api.endGame(id).catch(() => {})
  reset()
}

const start = (game) =>
  guarded(async () => {
    entries.value = []
    apply(await api.startGame(game, uiLanguage.value))
  })

const resume = () =>
  guarded(async () => {
    const saved = stored.value
    const state = await api.getGame(saved.id)
    uiLanguage.value = state.language
    session.value = { id: state.id, game: state.game, language: state.language, status: state.status }
    entries.value = saved.entries
  })

const command = (text) => guarded(async () => apply(await api.sendCommand(session.value.id, text), text))

const save = () =>
  guarded(async () => {
    const { blob, fileName } = await api.downloadTrack(session.value.id)
    const link = document.createElement('a')
    link.href = URL.createObjectURL(blob)
    link.download = fileName
    link.click()
    URL.revokeObjectURL(link.href)
    toast(texts[gameLanguage.value].saved)
  })

const load = (file) =>
  guarded(async () => {
    const response = await api.uploadTrack(session.value.id, file)
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
    @new="newGame"
  />
  <transition name="fade">
    <div v-if="notice" class="toast" role="status">{{ notice }}</div>
  </transition>
</template>

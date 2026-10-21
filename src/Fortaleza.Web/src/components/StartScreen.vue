<script setup>
import { texts } from '../i18n'

const modernPortUrl = 'https://fortressadventure.com/'

defineProps({ language: String, resumable: Object, busy: Boolean })
const emit = defineEmits(['start', 'resume', 'language', 'about', 'introduction', 'advices', 'settings'])
</script>

<template>
  <main class="start">
    <header class="hero">
      <div class="crest" aria-hidden="true">🏰</div>
      <h1>{{ texts[language].title }}</h1>
      <p>{{ texts[language].subtitle }}</p>
      <p class="modern-port">
        {{ texts[language].modernPortPre }}<a :href="modernPortUrl" target="_blank" rel="noopener">{{ modernPortUrl }}</a>{{ texts[language].modernPortPost }}
      </p>
    </header>

    <div class="lang" role="group" :aria-label="texts[language].language">
      <button :class="{ on: language === 'es' }" @click="emit('language', 'es')">Español</button>
      <button :class="{ on: language === 'en' }" @click="emit('language', 'en')">English</button>
    </div>

    <button v-if="resumable" class="card resume" :disabled="busy" @click="emit('resume')">
      <strong>{{ texts[language].continue }}</strong>
      <span>{{ resumable.game === 1 ? texts[language].game1 : texts[language].game2 }}</span>
    </button>

    <h2>{{ texts[language].chooseGame }}</h2>
    <div class="cards">
      <button class="card" :disabled="busy" @click="emit('start', 1)">
        <span class="numeral">I</span>
        <strong>{{ texts[language].game1 }}</strong>
        <span>{{ texts[language].game1Sub }}</span>
      </button>
      <button class="card" :disabled="busy" @click="emit('start', 2)">
        <span class="numeral">II</span>
        <strong>{{ texts[language].game2 }}</strong>
        <span>{{ texts[language].game2Sub }}</span>
      </button>
    </div>

    <footer class="colophon">
      <button class="link" @click="emit('introduction', 1)">📜 {{ texts[language].introduction }} I</button>
      <button class="link" @click="emit('introduction', 2)">📜 {{ texts[language].introduction }} II</button>
      <button class="link" @click="emit('advices')">💡 {{ texts[language].advices }}</button>
      <button class="link" @click="emit('settings')">⚙️ {{ texts[language].settings }}</button>
      <button class="link" @click="emit('about')">ⓘ {{ texts[language].about }}</button>
    </footer>
  </main>
</template>

<script setup>
import { ref } from 'vue'
import { texts } from '../i18n'

defineProps({ language: String })

const dialog = ref(null)
const version = __APP_VERSION__
const sourceUrl = 'https://github.com/rsotolongo/fortaleza-dotnet'
const licenseUrl = 'https://www.gnu.org/licenses/gpl-3.0.html'

function show() {
  if (!dialog.value.open) dialog.value.showModal()
}

function backdrop(event) {
  if (event.target === dialog.value) dialog.value.close()
}

defineExpose({ show })
</script>

<template>
  <dialog ref="dialog" class="about" aria-labelledby="about-title" @click="backdrop">
    <div class="about-body">
      <header>
        <div class="crest" aria-hidden="true">🏰</div>
        <h2 id="about-title">{{ texts[language].title }}</h2>
        <p class="version">{{ texts[language].version }} {{ version }}</p>
      </header>

      <dl>
        <dt>{{ texts[language].originalGame }}</dt>
        <dd>Miguel Cepero</dd>
        <dt>{{ texts[language].port }}</dt>
        <dd>Ricardo Sotolongo</dd>
        <dt>{{ texts[language].license }}</dt>
        <dd>
          <a :href="licenseUrl" target="_blank" rel="noopener">GNU GPL v3+</a>
          <small>{{ texts[language].licenseNote }}</small>
        </dd>
        <dt>{{ texts[language].source }}</dt>
        <dd>
          <a class="repo" :href="sourceUrl" target="_blank" rel="noopener">
            <svg viewBox="0 0 16 16" width="18" height="18" aria-hidden="true">
              <path
                fill="currentColor"
                d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.013 8.013 0 0016 8c0-4.42-3.58-8-8-8z"
              />
            </svg>
            https://github.com/rsotolongo/fortaleza-dotnet
          </a>
        </dd>
      </dl>

      <form method="dialog">
        <button class="primary" autofocus>{{ texts[language].close }}</button>
      </form>
    </div>
  </dialog>
</template>

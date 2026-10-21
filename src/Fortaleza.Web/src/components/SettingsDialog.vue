<script setup>
import { computed, ref } from 'vue'
import { texts } from '../i18n'
import { SCROLL, TYPEWRITE, interaction, setInteraction } from '../interaction'
import { applyTheme, loadTheme, saveTheme, themes } from '../themes'
import SelectField from './SelectField.vue'

const props = defineProps({ language: String })

const dialog = ref(null)
const theme = ref(loadTheme())

const themeOptions = computed(() => Object.entries(themes).map(([id, entry]) => ({ id, label: entry.names[props.language] })))
const interactionOptions = computed(() => [
  { id: SCROLL, label: texts[props.language].interactionScroll },
  { id: TYPEWRITE, label: texts[props.language].interactionTypewrite }
])

function chooseTheme(id) {
  theme.value = id
  applyTheme(id)
  saveTheme(id)
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

      <SelectField :label="texts[language].theme" :options="themeOptions" :model-value="theme" @update:model-value="chooseTheme" />
      <SelectField
        :label="texts[language].interaction"
        :options="interactionOptions"
        :model-value="interaction"
        @update:model-value="setInteraction"
      />

      <form method="dialog">
        <button class="primary" autofocus>{{ texts[language].close }}</button>
      </form>
    </div>
  </dialog>
</template>

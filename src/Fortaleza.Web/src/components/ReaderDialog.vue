<script setup>
import { computed, ref } from 'vue'
import { content } from '../content'
import { texts } from '../i18n'

const props = defineProps({ language: String })

const dialog = ref(null)
const kind = ref('advices')
const game = ref(1)

const title = computed(() => {
  const t = texts[props.language]
  if (kind.value === 'advices') return t.advices
  return `${t.introduction} · ${game.value === 1 ? t.game1 : t.game2}`
})

const blocks = computed(() => {
  const source = kind.value === 'advices' ? content.advices : content.introduction[game.value]
  const result = []
  for (const line of source[props.language]) {
    const last = result[result.length - 1]
    if (line.startsWith('## ')) result.push({ type: 'h3', text: line.slice(3) })
    else if (line.startsWith('- ')) {
      if (last?.type === 'ul') last.items.push(line.slice(2))
      else result.push({ type: 'ul', items: [line.slice(2)] })
    } else if (line.startsWith('> ')) result.push({ type: 'quote', text: line.slice(2) })
    else result.push({ type: 'p', text: line })
  }
  return result
})

function show(which, number = 1) {
  kind.value = which
  game.value = number
  if (!dialog.value.open) dialog.value.showModal()
  dialog.value.scrollTop = 0
}

function backdrop(event) {
  if (event.target === dialog.value) dialog.value.close()
}

defineExpose({ show })
</script>

<template>
  <dialog ref="dialog" class="reader" aria-labelledby="reader-title" @click="backdrop">
    <div class="reader-body">
      <header>
        <h2 id="reader-title">{{ title }}</h2>
        <form method="dialog">
          <button class="icon" :aria-label="texts[language].close" autofocus>✕</button>
        </form>
      </header>
      <article>
        <template v-for="(block, index) in blocks" :key="index">
          <h3 v-if="block.type === 'h3'">{{ block.text }}</h3>
          <ul v-else-if="block.type === 'ul'">
            <li v-for="(item, i) in block.items" :key="i">{{ item }}</li>
          </ul>
          <blockquote v-else-if="block.type === 'quote'">{{ block.text }}</blockquote>
          <p v-else>{{ block.text }}</p>
        </template>
      </article>
    </div>
  </dialog>
</template>

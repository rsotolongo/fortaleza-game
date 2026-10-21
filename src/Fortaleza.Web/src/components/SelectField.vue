<script setup>
import { computed, ref, useId } from 'vue'

const props = defineProps({ label: String, options: Array, modelValue: String })
const emit = defineEmits(['update:modelValue'])

const labelId = useId()
const open = ref(false)
const trigger = ref(null)

const current = computed(() => props.options.find((option) => option.id === props.modelValue))

function choose(id) {
  open.value = false
  trigger.value.focus()
  emit('update:modelValue', id)
}

function step(delta) {
  const ids = props.options.map((option) => option.id)
  choose(ids[(ids.indexOf(props.modelValue) + delta + ids.length) % ids.length])
}

function leave(event) {
  if (!event.currentTarget.contains(event.relatedTarget)) open.value = false
}

function escape(event) {
  if (!open.value) return
  event.stopPropagation()
  event.preventDefault()
  open.value = false
  trigger.value.focus()
}
</script>

<template>
  <div class="field">
    <span :id="labelId">{{ label }}</span>
    <div class="select" @focusout="leave">
      <button
        ref="trigger"
        type="button"
        class="trigger"
        aria-haspopup="listbox"
        :aria-labelledby="labelId"
        :aria-expanded="open"
        @click="open = !open"
        @keydown.down.prevent="step(1)"
        @keydown.up.prevent="step(-1)"
        @keydown.esc="escape"
      >
        <span>{{ current?.label }}</span>
        <svg viewBox="0 0 12 8" width="12" height="8" aria-hidden="true">
          <path d="M1 1.5l5 5 5-5" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" />
        </svg>
      </button>
      <ul v-if="open" role="listbox" :aria-labelledby="labelId" @mousedown.prevent @keydown.esc="escape">
        <li v-for="option in options" :key="option.id" role="none">
          <button type="button" role="option" :aria-selected="option.id === modelValue" @click="choose(option.id)">
            {{ option.label }}
            <span v-if="option.id === modelValue" aria-hidden="true">✓</span>
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

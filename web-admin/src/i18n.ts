import i18next from 'i18next'
import LanguageDetector from 'i18next-browser-languagedetector'
import { initReactI18next } from 'react-i18next'
import commonEn from '@/messages/en/common.json'
import commonEs from '@/messages/es/common.json'
import commonJa from '@/messages/ja/common.json'
import commonKo from '@/messages/ko/common.json'
import commonVi from '@/messages/vi/common.json'
import commonZh from '@/messages/zh/common.json'

// 6 ngôn ngữ giống web/ (ADR-0006), nhưng dùng react-i18next thay next-intl vì đây là Vite SPA,
// không phải Next.js — không có routing theo URL, lựa chọn lưu localStorage (xem ADR-0010).
export const SUPPORTED_LOCALES = ['vi', 'en', 'ja', 'zh', 'ko', 'es'] as const
export type SupportedLocale = (typeof SUPPORTED_LOCALES)[number]

i18next
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      vi: { common: commonVi },
      en: { common: commonEn },
      ja: { common: commonJa },
      zh: { common: commonZh },
      ko: { common: commonKo },
      es: { common: commonEs },
    },
    fallbackLng: 'vi',
    supportedLngs: SUPPORTED_LOCALES,
    ns: ['common'],
    defaultNS: 'common',
    detection: {
      order: ['localStorage', 'navigator'],
      lookupLocalStorage: 'blousehiding-admin-locale',
      caches: ['localStorage'],
    },
    interpolation: {
      escapeValue: false,
    },
  })

export default i18next

import i18next from 'i18next'
import LanguageDetector from 'i18next-browser-languagedetector'
import { initReactI18next } from 'react-i18next'
import commonEn from '@/messages/en/common.json'
import commonEs from '@/messages/es/common.json'
import commonJa from '@/messages/ja/common.json'
import commonKo from '@/messages/ko/common.json'
import commonVi from '@/messages/vi/common.json'
import commonZh from '@/messages/zh/common.json'
import jobsEn from '@/messages/en/jobs.json'
import jobsEs from '@/messages/es/jobs.json'
import jobsJa from '@/messages/ja/jobs.json'
import jobsKo from '@/messages/ko/jobs.json'
import jobsVi from '@/messages/vi/jobs.json'
import jobsZh from '@/messages/zh/jobs.json'
import applicationsEn from '@/messages/en/applications.json'
import applicationsEs from '@/messages/es/applications.json'
import applicationsJa from '@/messages/ja/applications.json'
import applicationsKo from '@/messages/ko/applications.json'
import applicationsVi from '@/messages/vi/applications.json'
import applicationsZh from '@/messages/zh/applications.json'
import creditEn from '@/messages/en/credit.json'
import creditEs from '@/messages/es/credit.json'
import creditJa from '@/messages/ja/credit.json'
import creditKo from '@/messages/ko/credit.json'
import creditVi from '@/messages/vi/credit.json'
import creditZh from '@/messages/zh/credit.json'
import candidatesEn from '@/messages/en/candidates.json'
import candidatesEs from '@/messages/es/candidates.json'
import candidatesJa from '@/messages/ja/candidates.json'
import candidatesKo from '@/messages/ko/candidates.json'
import candidatesVi from '@/messages/vi/candidates.json'
import candidatesZh from '@/messages/zh/candidates.json'
import membersEn from '@/messages/en/members.json'
import membersEs from '@/messages/es/members.json'
import membersJa from '@/messages/ja/members.json'
import membersKo from '@/messages/ko/members.json'
import membersVi from '@/messages/vi/members.json'
import membersZh from '@/messages/zh/members.json'
import opsEn from '@/messages/en/ops.json'
import opsEs from '@/messages/es/ops.json'
import opsJa from '@/messages/ja/ops.json'
import opsKo from '@/messages/ko/ops.json'
import opsVi from '@/messages/vi/ops.json'
import opsZh from '@/messages/zh/ops.json'

// 6 ngôn ngữ giống web/ (ADR-0006), nhưng dùng react-i18next thay next-intl vì đây là Vite SPA,
// không phải Next.js — không có routing theo URL, lựa chọn lưu localStorage (xem ADR-0010).
export const SUPPORTED_LOCALES = ['vi', 'en', 'ja', 'zh', 'ko', 'es'] as const
export type SupportedLocale = (typeof SUPPORTED_LOCALES)[number]

export const NAMESPACES = ['common', 'jobs', 'applications', 'credit', 'candidates', 'members', 'ops'] as const

i18next
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      vi: { common: commonVi, jobs: jobsVi, applications: applicationsVi, credit: creditVi, candidates: candidatesVi, members: membersVi, ops: opsVi },
      en: { common: commonEn, jobs: jobsEn, applications: applicationsEn, credit: creditEn, candidates: candidatesEn, members: membersEn, ops: opsEn },
      ja: { common: commonJa, jobs: jobsJa, applications: applicationsJa, credit: creditJa, candidates: candidatesJa, members: membersJa, ops: opsJa },
      zh: { common: commonZh, jobs: jobsZh, applications: applicationsZh, credit: creditZh, candidates: candidatesZh, members: membersZh, ops: opsZh },
      ko: { common: commonKo, jobs: jobsKo, applications: applicationsKo, credit: creditKo, candidates: candidatesKo, members: membersKo, ops: opsKo },
      es: { common: commonEs, jobs: jobsEs, applications: applicationsEs, credit: creditEs, candidates: candidatesEs, members: membersEs, ops: opsEs },
    },
    fallbackLng: 'vi',
    supportedLngs: SUPPORTED_LOCALES,
    ns: NAMESPACES,
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

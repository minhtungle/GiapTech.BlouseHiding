import { defineRouting } from "next-intl/routing";

export const LOCALES = ["vi", "en", "ja", "zh", "ko", "es"] as const;
export type Locale = (typeof LOCALES)[number];

export const LOCALE_LABELS: Record<Locale, string> = {
  vi: "Tiếng Việt",
  en: "English",
  ja: "日本語",
  zh: "中文",
  ko: "한국어",
  es: "Español",
};

// Xem ADR-0006: 6 ngôn ngữ, routing tiền tố URL luôn hiển thị, vi mặc định.
export const routing = defineRouting({
  locales: LOCALES,
  defaultLocale: "vi",
  localePrefix: "always",
});

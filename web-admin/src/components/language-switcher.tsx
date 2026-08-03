import { Check, Globe } from 'lucide-react'
import { useTranslation } from 'react-i18next'
import { cn } from '@/lib/utils'
import { SUPPORTED_LOCALES, type SupportedLocale } from '@/i18n'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'

// Tên bản ngữ, không dịch sang ngôn ngữ đang xem — cùng nguyên tắc với LanguageSwitcher ở web/
// (ADR-0006 mục 3).
const LOCALE_LABEL: Record<SupportedLocale, string> = {
  vi: 'Tiếng Việt',
  en: 'English',
  ja: '日本語',
  zh: '中文',
  ko: '한국어',
  es: 'Español',
}

export function LanguageSwitcher() {
  const { i18n } = useTranslation()
  const currentLocale = (i18n.resolvedLanguage ?? 'vi') as SupportedLocale

  return (
    <DropdownMenu modal={false}>
      <DropdownMenuTrigger asChild>
        <Button variant='ghost' size='icon' className='scale-95 rounded-full'>
          <Globe className='size-[1.2rem]' />
          <span className='sr-only'>{LOCALE_LABEL[currentLocale]}</span>
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align='end'>
        {SUPPORTED_LOCALES.map((locale) => (
          <DropdownMenuItem key={locale} onClick={() => i18n.changeLanguage(locale)}>
            {LOCALE_LABEL[locale]}
            <Check
              size={14}
              className={cn('ms-auto', locale !== currentLocale && 'hidden')}
            />
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  )
}

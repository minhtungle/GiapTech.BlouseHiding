import { useTranslations } from "next-intl";
import { LanguageSwitcher } from "@/components/language-switcher";

export function SiteFooter() {
  const t = useTranslations("common");

  return (
    <footer className="mt-auto border-t border-border bg-paper-raised">
      <div className="mx-auto flex max-w-6xl flex-col gap-4 px-4 py-8 text-sm text-ink-muted sm:flex-row sm:items-center sm:justify-between sm:px-6">
        <p>
          © {new Date().getFullYear()} {t("brand")} — {t("footer.rights")}
        </p>
        <div className="flex items-center gap-2">
          <span>{t("footer.language")}:</span>
          <LanguageSwitcher />
        </div>
      </div>
    </footer>
  );
}

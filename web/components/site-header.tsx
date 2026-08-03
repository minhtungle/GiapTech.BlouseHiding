import { getTranslations } from "next-intl/server";
import { Link } from "@/i18n/navigation";
import { Button } from "@/components/ui/button";
import { LanguageSwitcher } from "@/components/language-switcher";
import { getCurrentUser } from "@/lib/session";
import { SiteHeaderUserMenu } from "@/components/site-header-user-menu";

export async function SiteHeader() {
  const t = await getTranslations("common");
  const currentUser = await getCurrentUser();

  return (
    <header className="sticky top-0 z-40 border-b border-border bg-paper-raised/95 backdrop-blur">
      <div className="mx-auto flex h-16 max-w-6xl items-center justify-between px-4 sm:px-6">
        <Link href="/" className="font-heading text-xl font-semibold text-ink">
          {t("brand")}
        </Link>

        <nav className="hidden items-center gap-6 text-sm text-ink-muted md:flex">
          <Link href="/jobs" className="hover:text-ink">
            {t("nav.jobs")}
          </Link>
          <Link href="/organizations" className="hover:text-ink">
            {t("nav.organizations")}
          </Link>
          <Link href="/about" className="hover:text-ink">
            {t("nav.about")}
          </Link>
        </nav>

        <div className="flex items-center gap-2">
          <LanguageSwitcher />
          {currentUser ? (
            <SiteHeaderUserMenu email={currentUser.email} />
          ) : (
            <>
              <Button variant="ghost" size="sm" asChild>
                <Link href="/auth/login">{t("nav.login")}</Link>
              </Button>
              <Button size="sm" asChild>
                <Link href="/auth/register">{t("nav.register")}</Link>
              </Button>
            </>
          )}
        </div>
      </div>
    </header>
  );
}

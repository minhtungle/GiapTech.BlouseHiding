import { useTranslations } from "next-intl";
import { Link } from "@/i18n/navigation";
import { cn } from "@/lib/utils";

export function AccountNav({ active }: { active: "dashboard" | "profile" | "settings" }) {
  const t = useTranslations("profile");

  const items = [
    { key: "dashboard" as const, href: "/dashboard", label: t("tabOverview") },
    { key: "profile" as const, href: "/profile", label: t("navLabel") },
    { key: "settings" as const, href: "/settings", label: t("tabSettings") },
  ];

  return (
    <nav className="flex gap-1 border-b border-border pb-0">
      {items.map((item) => (
        <Link
          key={item.key}
          href={item.href}
          className={cn(
            "rounded-t-md border-b-2 px-3 py-2 text-sm font-medium transition-colors",
            active === item.key
              ? "border-accent-seal text-ink"
              : "border-transparent text-ink-muted hover:text-ink",
          )}
        >
          {item.label}
        </Link>
      ))}
    </nav>
  );
}

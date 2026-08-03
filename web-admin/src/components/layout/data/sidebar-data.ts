import {
  Construction,
  LayoutDashboard,
  Monitor,
  Bug,
  FileX,
  HelpCircle,
  Lock,
  Bell,
  Palette,
  ServerOff,
  Settings,
  Wrench,
  UserCog,
  UserX,
  Users,
  MessagesSquare,
  ShieldCheck,
  Briefcase,
  Wallet,
  ClipboardCheck,
  Landmark,
  Flag,
  BookMarked,
  UserSearch,
} from 'lucide-react'
import { type SidebarData } from '../types'

// title/plan dưới đây là KEY dịch (namespace "common", xem src/messages/) — không phải chuỗi hiển
// thị trực tiếp. NavGroup/nav-group.tsx phải gọi qua t(item.title), không render item.title thẳng.
// TODO (Giai đoạn 0.2): tách nav theo role thật (Admin NTD vs Vận hành) khi nối
// auth thật — hiện tại là placeholder chung cho cả 2 vai trò, xem ADR-0008.
export const sidebarData: SidebarData = {
  user: {
    name: 'Người dùng demo',
    email: 'demo@blousehiding.vn',
    avatar: '/avatars/shadcn.jpg',
  },
  teams: [
    {
      name: 'BlouseHiding',
      logo: Briefcase,
      plan: 'brandPlan',
    },
  ],
  navGroups: [
    {
      title: 'nav.employerGroup',
      items: [
        {
          title: 'nav.dashboard',
          url: '/',
          icon: LayoutDashboard,
        },
        {
          title: 'nav.jobs',
          url: '/jobs',
          icon: Briefcase,
        },
        {
          title: 'nav.ats',
          url: '/jobs',
          icon: ClipboardCheck,
        },
        {
          title: 'nav.credit',
          url: '/credit',
          icon: Wallet,
        },
        {
          title: 'nav.candidates',
          url: '/candidates',
          icon: UserSearch,
        },
        {
          title: 'nav.members',
          url: '/users',
          icon: Users,
        },
        {
          title: 'nav.chats',
          url: '/chats',
          badge: '3',
          icon: MessagesSquare,
        },
      ],
    },
    {
      title: 'nav.opsGroup',
      items: [
        {
          title: 'nav.opsVerification',
          url: '/ops/verification',
          icon: ShieldCheck,
        },
        {
          title: 'nav.opsPayments',
          url: '/ops/payments',
          icon: Landmark,
        },
        {
          title: 'nav.opsReports',
          url: '/ops/reports',
          icon: Flag,
        },
        {
          title: 'nav.opsCatalog',
          url: '/ops/catalog',
          icon: BookMarked,
        },
      ],
    },
    {
      title: 'Pages',
      items: [
        {
          title: 'Auth',
          icon: ShieldCheck,
          items: [
            {
              title: 'Sign In',
              url: '/sign-in',
            },
            {
              title: 'Sign In (2 Col)',
              url: '/sign-in-2',
            },
            {
              title: 'Sign Up',
              url: '/sign-up',
            },
            {
              title: 'Forgot Password',
              url: '/forgot-password',
            },
            {
              title: 'OTP',
              url: '/otp',
            },
          ],
        },
        {
          title: 'Errors',
          icon: Bug,
          items: [
            {
              title: 'Unauthorized',
              url: '/errors/unauthorized',
              icon: Lock,
            },
            {
              title: 'Forbidden',
              url: '/errors/forbidden',
              icon: UserX,
            },
            {
              title: 'Not Found',
              url: '/errors/not-found',
              icon: FileX,
            },
            {
              title: 'Internal Server Error',
              url: '/errors/internal-server-error',
              icon: ServerOff,
            },
            {
              title: 'Maintenance Error',
              url: '/errors/maintenance-error',
              icon: Construction,
            },
          ],
        },
      ],
    },
    {
      title: 'Other',
      items: [
        {
          title: 'Settings',
          icon: Settings,
          items: [
            {
              title: 'Profile',
              url: '/settings',
              icon: UserCog,
            },
            {
              title: 'Account',
              url: '/settings/account',
              icon: Wrench,
            },
            {
              title: 'Appearance',
              url: '/settings/appearance',
              icon: Palette,
            },
            {
              title: 'Notifications',
              url: '/settings/notifications',
              icon: Bell,
            },
            {
              title: 'Display',
              url: '/settings/display',
              icon: Monitor,
            },
          ],
        },
        {
          title: 'Help Center',
          url: '/help-center',
          icon: HelpCircle,
        },
      ],
    },
  ],
}

import { cn } from "@/lib/utils";

export function Container({
  children,
  className,
}: {
  children: React.ReactNode;
  className?: string;
}) {
  return (
    // <div className="max-w-[1200px] mx-auto grid grid-cols-12 gap-6 px-4 py-3">
    //   {children}
    // </div>
    <div className={cn("max-w-screen-xl mx-auto px-4", className)}>
      {children}
    </div>
  );
}

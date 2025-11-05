export function Container({ children }: { children: React.ReactNode }) {
  return (
    <div className="max-w-[1200px] mx-auto grid grid-cols-12 gap-6 px-4 py-3">
      {children}
    </div>
  );
}

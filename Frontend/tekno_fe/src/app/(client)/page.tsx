import Login from "@/components/auth/Login";

// HomePage
export default function Home() {
  return (
    <div className="px-2 pb-3 md: px-0">
      <div className="">Landing</div>
      <div className="breadcrumbs text-sm">
        <ul>
          <li>
            <a>Home</a>
          </li>
          <li>
            <a>Documents</a>
          </li>
          <li>Add Document</li>
        </ul>
      </div>
    </div>
  );
}

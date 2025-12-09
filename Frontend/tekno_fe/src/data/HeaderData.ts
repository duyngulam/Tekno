import { getCategoriesList } from "@/services/categories";

const data = await getCategoriesList();
const category = data[0].slug
export const headerData = [
  { href: "/", match: "/", label: "Home" },
  { href: `/products?category=${category}`, match: "/products", label: "Products" },
  { href: "/blogs", match: "/blogs", label: "Blogs" },
  { href: "/faq", match: "/faq", label: "FAQ" },
  { href: "/contact-us", match: "/contact-us", label: "Contact us" },
];


import type { PaginatedResponse } from "@/types/Pagination";
import type {
  ProductResponse,
  ProductWithPricesResponse,
} from "@/types/Product";

export async function getProducts(
  page: number,
  search: string | undefined
): Promise<PaginatedResponse<ProductResponse>> {
  const url = new URL("/products", import.meta.env.API_URL);
  url.searchParams.append("p", page.toString());
  url.searchParams.append("s", "100");

  if (search && search.length > 0) {
    url.searchParams.append("q", search);
  }

  const response = await fetch(url);

  return await response.json();
}

export async function getProduct(
  storeId: number,
  sku: string
): Promise<ProductWithPricesResponse | undefined> {
  const url = new URL(`/products/${storeId}/${sku}`, import.meta.env.API_URL);
  const response = await fetch(url);

  if (!response.ok) {
    return undefined;
  }

  return await response.json();
}

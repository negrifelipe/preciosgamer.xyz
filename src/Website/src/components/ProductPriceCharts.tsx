import type { ProductPriceResponse } from "@/types/Product";
import PriceChart from "./PriceChart";
import { useState } from "react";

export default function ProductPriceCharts({
  defaultPrices,
}: {
  defaultPrices: ProductPriceResponse[];
}) {
  const [prices, setPrices] = useState<ProductPriceResponse[]>(defaultPrices);

  // TODO: Add a way to add prices from the backend so that we can display + last 30 days.
  // There should be a select with for example "Ultimos 30 daias", "Ultimos 60 dias" and "Desde el comienzo". The prices should be fetched from the backend and then set in the state.

  return (
    <section>
      <h2 className="text-xl mb-2">Precio</h2>
      <PriceChart prices={prices} />
    </section>
  );
}

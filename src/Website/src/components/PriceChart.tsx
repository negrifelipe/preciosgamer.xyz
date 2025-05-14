import type { ProductPriceResponse } from "@/types/Product";
import { useEffect, useState } from "react";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from "recharts";

export default function PriceChart({
  prices,
}: {
  prices: ProductPriceResponse[];
}) {
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(0);

  useEffect(() => {
    const max = Math.max(...prices.map((p) => p.price));
    const min = Math.min(...prices.map((p) => p.price));

    const range = max - min;
    const base = Math.pow(10, Math.floor(Math.log10(range)));

    // Round down min and round up max to the nearest base
    const scaledMax = Math.ceil(max / base) * base;
    const scaledMin = Math.floor(min / base) * base;

    setMinPrice(scaledMin);
    setMaxPrice(scaledMax);
  }, [prices]);

  const formatter = (value: number) =>
    new Intl.NumberFormat("en-US").format(value);

  return (
    <ResponsiveContainer width="100%" height={300}>
      <LineChart
        className="text-black"
        data={prices.reverse().map((p) => {
          return {
            name: new Date(p.priceDate).toLocaleDateString("es-AR", {
              day: "2-digit",
              month: "2-digit",
            }),
            price: p.price,
          };
        })}
      >
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="name" />
        <YAxis
          type="number"
          domain={[minPrice, maxPrice]}
          tickFormatter={formatter}
        />
        <Tooltip formatter={formatter} />
        <Legend />
        <Line
          type="monotone"
          dataKey="price"
          name="Precio ARS"
          stroke="#82ca9d"
        />
      </LineChart>
    </ResponsiveContainer>
  );
}

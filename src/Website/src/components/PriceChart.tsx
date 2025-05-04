import type { ProductPriceResponse } from "@/types/Product";
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
        <YAxis />
        <Tooltip />
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

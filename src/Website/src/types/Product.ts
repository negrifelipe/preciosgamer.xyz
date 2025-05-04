export interface ProductResponse {
  sku: string;
  storeId: number;
  createDate: Date;
  details: ProductDetails;
}

export interface ProductDetails {
  name: string;
  url: string;
  imageUrl: string | undefined;
  price: number;
}

export interface ProductPriceResponse {
  productSKU: string;
  storeId: number;
  price: number;
  priceDate: Date;
}

export interface ProductWithPricesResponse {
  sku: string;
  storeId: number;
  createDate: Date;
  details: ProductDetails;
  prices: ProductPriceResponse[];
}

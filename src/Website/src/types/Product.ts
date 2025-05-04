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

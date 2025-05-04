export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

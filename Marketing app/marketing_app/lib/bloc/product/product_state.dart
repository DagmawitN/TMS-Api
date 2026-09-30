part of 'product_bloc.dart';

abstract class ProductState {}

class ProductInitial extends ProductState {}

class ProductLoading extends ProductState {}

class ProductList extends ProductState {
  final List<Product> products;

  ProductList({required this.products});
}

class ProductLoaded extends ProductState {
  final Product product;

  ProductLoaded({required this.product});
}

class ProductError extends ProductState {
  final String message;

  ProductError(this.message);
}

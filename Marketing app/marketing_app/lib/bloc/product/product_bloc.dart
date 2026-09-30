import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:marketing_app/models/product.dart';
import 'package:marketing_app/bloc/product/product_event.dart';
import 'package:marketing_app/data/product_data_provider.dart';

part 'product_state.dart';

class ProductBloc extends Bloc<ProductEvent, ProductState> {
  final ProductDataProvider provider;

  ProductBloc({ProductDataProvider? dataProvider})
    : provider = dataProvider ?? ProductDataProvider(),
      super(ProductInitial()) {
    on<getProduct>((event, emit) async {
      emit(ProductLoading());
      try {
        final products = await provider.getProducts();
        emit(ProductList(products: products));
      } catch (error) {
        emit(ProductError(error.toString()));
      }
    });

    on<getProductbyId>((event, emit) async {
      emit(ProductLoading());
      try {
        final product = await provider.getProduct(event.productId);
        emit(ProductLoaded(product: product));
      } catch (error) {
        emit(ProductError(error.toString()));
      }
    });

    on<createProduct>((event, emit) async {
      emit(ProductLoading());
      try {
        await provider.createProduct(event.product);
        final products = await provider.getProducts();
        emit(ProductList(products: products));
      } catch (error) {
        emit(ProductError(error.toString()));
      }
    });

    on<updateProduct>((event, emit) async {
      emit(ProductLoading());
      try {
        await provider.updateProduct(event.product);
        final products = await provider.getProducts();
        emit(ProductList(products: products));
      } catch (error) {
        emit(ProductError(error.toString()));
      }
    });

    on<deleteProduct>((event, emit) async {
      emit(ProductLoading());
      try {
        await provider.deleteProduct(event.productId);
        final products = await provider.getProducts();
        emit(ProductList(products: products));
      } catch (error) {
        emit(ProductError(error.toString()));
      }
    });
  }
}

import 'package:flutter_bloc/flutter_bloc.dart';
import '../../data/market_store.dart';
import '../../data/product_data_provider.dart';
import 'cart_event.dart';
import 'cart_state.dart';

class CartBloc extends Bloc<CartEvent, CartState> {
  final ProductDataProvider provider;

  CartBloc({ProductDataProvider? dataProvider})
    : provider = dataProvider ?? ProductDataProvider(),
      super(CartState(items: List.from(MarketStore.cart))) {
    on<AddToCart>((event, emit) async {
      try {
        final product = await provider.getProduct(event.productId);
        MarketStore.addToCart(product, 1);

        emit(CartState(items: List.from(MarketStore.cart)));
      } catch (_) {
        return;
      }
    });

    on<RemoveFromCart>((event, emit) {
      MarketStore.removeFromCart(event.productId);

      emit(CartState(items: List.from(MarketStore.cart)));
    });
    on<CleanCart>((event, emit) {
      MarketStore.clearCart();
      emit(CartState(items: List.from(MarketStore.cart)));
    });
  }
}

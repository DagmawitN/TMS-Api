abstract class CartEvent {}

class AddToCart extends CartEvent {
  final String productId;

  AddToCart(this.productId);
}

class RemoveFromCart extends CartEvent {
  final String productId;

  RemoveFromCart(this.productId);
}

class CleanCart extends CartEvent {}

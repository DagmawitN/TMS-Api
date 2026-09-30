import '../../models/product.dart';

class CartState {
  final List<CartItem> items;

  CartState({
    required this.items,
  });

  int get itemCount {
    int count = 0;

    for (final item in items) {
      count += item.quantity;
    }

    return count;
  }

  double get total {
    double total = 0;

    for (final item in items) {
      total += item.total;
    }

    return total;
  }
}
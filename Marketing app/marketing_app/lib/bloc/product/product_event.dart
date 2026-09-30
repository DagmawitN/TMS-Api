import 'package:marketing_app/models/product.dart';

abstract class ProductEvent {

}

class getProduct extends ProductEvent {

  getProduct();
}

class getProductbyId extends ProductEvent {
  final String productId;

  getProductbyId(this.productId);
}

class createProduct extends ProductEvent {
  final Product product;
  

  createProduct({
    required this.product,
  });
} 

class updateProduct extends ProductEvent {
  final Product product;

  updateProduct({
    required this.product,
  });
}

class deleteProduct extends ProductEvent {
  final String productId;

  deleteProduct({
    required this.productId,
  });
}
import 'dart:convert';

import '../models/product.dart';
import 'package:http/http.dart' as http;

class MarketStore {
  MarketStore._();


  static final List<Product> products = [
    const Product(
      id: 'p1',
      title: 'Phone X',
      price: 549,
      category: 'smartphones',
      description: '6.1-inch display, 128 GB storage, dual camera.',
    ),
    const Product(
      id: 'p2',
      title: 'Headphones',
      price: 89,
      category: 'audio',
      description: 'Over-ear headphones with 30 hours of battery life.',
    ),
    const Product(
      id: 'p3',
      title: 'T-shirt',
      price: 15,
      category: 'clothing',
      description: '100% cotton, regular fit, machine washable.',
    ),
    const Product(
      id: 'p4',
      title: 'Laptop',
      price: 899,
      category: 'computers',
      description: '14-inch laptop, 16 GB RAM, 512 GB SSD.',
    ),
    const Product(
      id: 'p5',
      title: 'Camera',
      price: 320,
      category: 'photography',
      description: 'Compact camera with 20x optical zoom.',
    ),
    const Product(
      id: 'p6',
      title: 'Backpack',
      price: 42,
      category: 'accessories',
      description: 'Water resistant backpack with a laptop pocket.',
    ),
  ];

  static final List<CartItem> cart = [];

  Future <Product?> findProduct(String id) async {
    
    try{
      final response =await http.get(Uri.parse('https://dummyjson.com/products/$id'));
      if (response.statusCode == 200 ){
        return Product.fromJson(response.body as Map<String,dynamic>);
      }
    }catch (e){
      throw Exception (e.toString());
    }

    return null;
  }

  Future <Product?> getProducts() async{
    try{
      final response = await http.get(
      Uri.parse('https://dummyjson.com/products'),
    );
    if (response.statusCode == 200){
      final products = [];
      final body = response.body as List<Map<String,String>>;
      for (var product in body){
        print(product);
        products.add(Product.fromJson(product));
      }
    }
    }catch(e){
      throw Exception(e.toString());
    }

  }

  Future<Product?> addProduct(Product product) async {

    try {
      final response = await http.post(
      Uri.parse('https://dummyjson.com/products/add'),
      headers: {'Content-Type': 'application/json'},
      body: product.toJson(),
    );
    return Product.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
    }catch (e){
      throw Exception("Failed to create product");
    }
  }

  Future<Product?> updateProduct(Product product) async{
    try{
      final response = await http.put(
        Uri.parse('https://dummyjson.com/products/${product.id}'),
        headers: {'Content-Type': 'application/json'},
        body: product.toJson(),
      );
      if (response.statusCode == 200){
        return Product.fromJson(jsonDecode(response.body) as Map<String, dynamic>);
      }
    }catch(e){
      throw Exception("Failed ot update product");
    }
  }

  static void deleteProduct(String id) async{
   try{
    await http.delete(Uri.parse('https://dummyjson.com/products/$id'));
   }catch(e){
    throw Exception (e.toString());
   }
  }



  static String newProductId() {
    return 'p${DateTime.now().millisecondsSinceEpoch}';
  }

  static void addToCart(Product product, int quantity) {
    final index = cart.indexWhere((item) => item.product.id == product.id);
    if (index == -1) {
      cart.add(CartItem(product: product, quantity: quantity));
    } else {
      cart[index].quantity += quantity;
    }
  }

  static void removeFromCart(String productId) {
    cart.removeWhere((item) => item.product.id == productId);
  }

  static void clearCart() {
    cart.clear();
  }

  static int get cartCount {
    int count = 0;
    for (final item in cart) {
      count += item.quantity;
    }
    return count;
  }

  static double get cartTotal {
    double total = 0;
    for (final item in cart) {
      total += item.total;
    }
    return total;
  }
}

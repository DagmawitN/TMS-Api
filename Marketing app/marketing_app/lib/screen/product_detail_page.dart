// import 'package:flutter/material.dart';
// import 'package:marketing_app/data/categories.dart';
// import 'package:marketing_app/data/market_store.dart';

// class ProductDetail extends StatelessWidget {
//   final String productId;

//   const ProductDetail({super.key, required this.productId});

//   @override
//   Widget build(BuildContext context) {
//     final product = MarketStore.findProduct(productId);

//     if (product == null) {
//       return Scaffold(
//         appBar: AppBar(title: const Text('Product Details')),
//         body: const Center(child: Text('Product not found.')),
//       );
//     }

//     final productColor = colorForCategory(product.category);
//     final productIcon = iconForCategory(product.category);

//     return Scaffold(
//       appBar: AppBar(
//         title: Text(product.title),
//         actions: [
//           IconButton(
//             icon: const Icon(Icons.edit),
//             onPressed: () {
//               // Navigate to the cart page
//               Navigator.pushNamed(context, '/cart');
//             },
//           ),
//           IconButton(icon: const Icon(Icons.delete), onPressed: () {
//             // Handle delete action
//           }),
//         ],
      
//       ),
//       body: Column(
//         children: [
//           Container(
//             height: 200,
//             width: double.infinity,
//             color: productColor.withValues(alpha: 0.1),
//             child: Center(
//               child: Icon(
//                 productIcon,
//                 size: 100,
//                 color: productColor,
//               ),
//             ),
//           ),

//           Text(product.title, style: const TextStyle(fontSize: 24)),
//           Text('\$${product.price.toStringAsFixed(2)}', style: const TextStyle(fontSize: 20)),
//           Text('Category: ${product.category}'),
//           Text(product.description, style: const TextStyle(color: Colors.grey)),
//           Row(children:[
//             Text("Qty", style: TextStyle(color: Colors.grey)),
//             IconButton(
//               icon: Icon(Icons.remove),
//               onPressed: () {
//                 // Decrease quantity lo gic
//               },
//             ),
//             Text("1"),
//             IconButton(
//               icon: Icon(Icons.add),
//               onPressed: () {
//                 // Increase quantity logic
//               },
//             ),
//           ]),

//           ElevatedButton(
//             style: ElevatedButton.styleFrom(
//               backgroundColor: Colors.blue,
//             ),
//             onPressed: () {
//               // Handle add to cart action
//             },
//             child: const Text('Save Product', style: TextStyle(color: Colors.white, fontWeight: FontWeight.bold)),
//           ),
//           // Add more product details as needed
//         ],
//       ),
//     );
//   }
// }
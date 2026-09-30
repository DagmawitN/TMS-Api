import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:marketing_app/bloc/cart/cart_bloc.dart';
import 'package:marketing_app/bloc/cart/cart_event.dart';
import 'package:marketing_app/data/categories.dart';
import 'package:marketing_app/models/product.dart';

class ProductCard extends StatelessWidget {
  final Product product;
  final VoidCallback onTap;

  const ProductCard({super.key, required this.product, required this.onTap});

  @override
  Widget build(BuildContext context) {
    final productColor = colorForCategory(product.category);
    final productIcon = iconForCategory(product.category);

    return InkWell(
      onTap: onTap,
      borderRadius: BorderRadius.circular(10),
      child: Container(
        padding: EdgeInsets.all(10),
        decoration: BoxDecoration(
          border: BoxBorder.all(color: Colors.grey.shade400),
          borderRadius: BorderRadius.circular(10),
        ),
        width: double.infinity,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Expanded(
              child: Container(
                width: double.infinity,
                height: 100,
                decoration: BoxDecoration(
                  color: productColor.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Icon(productIcon, size: 30, color: productColor),
              ),
            ),
            SizedBox(height: 10),
            Text(product.title, style: TextStyle(fontSize: 20)),
            SizedBox(height: 10),
            Text(
              product.price.toString(),
              style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
            ),
            SizedBox(height: 8),

            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: () {
                  context.read<CartBloc>().add(
                    AddToCart(product.id),
                  );
                },
                child: const Text('Add to Cart'),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

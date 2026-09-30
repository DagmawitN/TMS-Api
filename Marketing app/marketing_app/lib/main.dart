import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:marketing_app/bloc/cart/cart_bloc.dart';
import 'package:marketing_app/bloc/cart/cart_state.dart';
import 'package:go_router/go_router.dart';
import 'package:marketing_app/bloc/product/product_bloc.dart';
import 'package:marketing_app/bloc/product/product_event.dart';
import 'package:marketing_app/router.dart';
import 'package:marketing_app/screen/product_card.dart';

void main() {
  runApp(
    MultiBlocProvider(
      providers: [
        BlocProvider(
          create: (context) => CartBloc(),
        ),
        BlocProvider(
          create: (context) => ProductBloc()..add(getProduct()),
        ),
      ],
      child: MaterialApp.router(
        routerConfig: router,
        debugShowCheckedModeBanner: false,
      ),
    ),
  );
}

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {

  void _openCart() {
    context.go('/cart');
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Mini Market"),
        actions: [
          BlocBuilder<CartBloc, CartState>(
            builder: (context, state) {
              return Badge(
                isLabelVisible: state.itemCount > 0,
                label: Text(state.itemCount.toString()),
                child: IconButton(
                  onPressed: _openCart,
                  icon: const Icon(Icons.shopping_cart),
                ),
              );
            },
          ),
        ],
        actionsPadding: const EdgeInsets.all(8),
        bottom: const PreferredSize(
          preferredSize: Size.fromHeight(1),
          child: Divider(),
        ),
      ),

      body: BlocBuilder<ProductBloc, ProductState>(
        builder: (context, state) {

          if (state is ProductLoading) {
            return const Center(
              child: CircularProgressIndicator(),
            );
          }

          if (state is ProductList) {
            final products = state.products;

            return Padding(
              padding: const EdgeInsets.all(16.0),
              child: GridView.builder(
                itemCount: products.length,
                gridDelegate:
                    const SliverGridDelegateWithFixedCrossAxisCount(
                  crossAxisCount: 2,
                  mainAxisSpacing: 10,
                  crossAxisSpacing: 10,
                  childAspectRatio: 0.85,
                ),
                itemBuilder: (context, index) {
                  final product = products[index];

                  return ProductCard(
                    product: product,
                    onTap: () {
                      context.go('/product/${product.id}');
                    },
                  );
                },
              ),
            );
          }

          return const Center(
            child: Text("No products found"),
          );
        },
      ),

      floatingActionButton: FloatingActionButton(
        onPressed: () {
          context.go('/product/new');
        },
        child: const Icon(Icons.add),
      ),
    );
  }
}
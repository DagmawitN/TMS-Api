import 'package:go_router/go_router.dart';

import 'package:marketing_app/main.dart';
import 'package:marketing_app/screen/product_detail_page.dart';
import 'package:marketing_app/screen/cart_page.dart';
import 'package:marketing_app/screen/add_product.dart';
import 'package:marketing_app/screen/product_detail_screen.dart';

final GoRouter router = GoRouter(
  initialLocation: '/',
  routes: [
    GoRoute(
      path: '/',
      builder: (context, state) => const HomePage(),
    ),
    GoRoute (path: '/product/new', builder: (context, state) => const AddProduct(),
    ),
    GoRoute( path: '/product/:id', builder: (context, state) 
      {
        final String id = state.pathParameters['id']!;
        return ProductDetailScreen(productId: id);
      },
    ),
    
    GoRoute( path: '/cart', builder: (context, state) => const CartPage(),
    ),
    
    // GoRoute (path: '/product/:id/edit', builder: (context, state) 
    //{final String id = state.pathParameters['id']!;
    // final Product? product = MarketStore.findProduct(id); 
    //    return ProductForm(product: product);
    //  },
   // ),
  ],
);
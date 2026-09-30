import 'package:flutter/material.dart';
import 'package:marketing_app/data/categories.dart';

class AddProduct extends StatefulWidget {
  const AddProduct({super.key});
  @override
  State<AddProduct> createState() => _AddProductState();

  
  }


class _AddProductState extends State<AddProduct> {
  String selectedCategory = kCategories.first;
  @override
  
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        leading: IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: () => Navigator.of(context).pop(),
        ),
        title: const Text("Add Product"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(10),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              "Title",
              style: TextStyle(color: Colors.grey),
            ),
            const TextField(
              decoration: InputDecoration(
                border: OutlineInputBorder(),
                hintText: "Enter product title",
              ),
            ),
            const Text(
              "Price",
              style: TextStyle(color: Colors.grey),
            ),
            const TextField(
              decoration: InputDecoration(
                border: OutlineInputBorder(),
                hintText: "Enter product price",
              ),
            ),
            const Text  (
              "Category",
              style: TextStyle(color: Colors.grey),
            ),
            DropdownButton<String>(
              value: selectedCategory,
          
              isExpanded: true,

              items: kCategories.map((category) {
                return DropdownMenuItem<String>(
                  value: category,
                  child: Text(category),
                );
              }).toList(),

              onChanged: (value) {
                setState(() {
                  selectedCategory = value!;
                });
              },
            ),
            Text("Description", style: TextStyle(color: Colors.grey)),
            TextField(
              maxLines: 3,
              decoration: InputDecoration(
                border: OutlineInputBorder(),
                hintText: "Enter product description",
              ),
            ),
            SizedBox(height: 20),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.blue,
                ),
                onPressed: () {
                  // Handle product submission logic here
                },
                child: Text("Save Product", style: TextStyle(color: Colors.white , fontWeight: FontWeight.bold)),
              ),
            )

          ],
        ),
      ),
    );
  }
}


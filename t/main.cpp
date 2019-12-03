//#include "t.h"
#include <iostream>
using namespace std;
class A 
{
public:
	A(int a):a(a) {}
private:
	int a;
};

void show(A &a)
{
	cout << a.a << endl;
}
int main()
{
	A a(1);
	show(a);
	return 0;
}
//#include "t.h"
#include <iostream>
#include <stack>
using namespace std;
class Solution {
public:
	bool backspaceCompare(string S, string T) {
		stack<char> p1;
		stack<char> p2;
		for (char s : S) {
			if (s == '#' && !p1.empty()) {
				p1.pop();
			}
			if else(s == '#') {

			}
		}
		for (char s : T) {
			if (s == '#' && !p2.empty()) {
				p2.pop();
			}
			if else(s == '#' && p2.empty()) {
				continue;
			}
			else {
				p2.push(s);
			}
		}
		while (!p1.empty() && !p2.empty()) {
			if (p1.top() == p2.top()) {
				p1.pop();
				p2.pop();
			}
			else {
				break;
			}
		}
		return p1.empty() && p2.empty();
	}
};
int main()
{
	A a(1);
	show(a);
	return 0;
}
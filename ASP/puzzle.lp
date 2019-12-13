#const n = 3.

shape(cross;line;corner;empty).

coords(X, Y) :- X = (0..n)*5, Y = (0..n)*5.

path(XF, YF, X, Y) :-
	inst(XF, YF, TF),
	TF != empty,
 	inst(X, Y, T),
 	T != empty,
 	XF - 5 = X, YF = Y.

path(XF, YF, X, Y) :-
	inst(XF, YF, TF),
	TF != empty,
 	inst(X, Y, T),
 	T != empty,
 	XF = X, YF - 5 = Y.

path(XF, YF, X, Y) :-
	inst(XF, YF, TF),
	TF != empty,
 	inst(XI, YI, TI),
 	TI != empty,
 	inst(X, Y, T),
 	T != empty,
 	path(XF, YF, XI, YI),
 	path(XI, YI, X, Y).


1 {inst(X, Y, T): shape(T)} 1 :- coords(X, Y).

:- 	inst(X, Y, T), T != empty,
	I=(-1..1)*5, I != 0,
	inst(X+I, Y, T),
	inst(X-I, Y, T).

:- 	inst(X, Y, T), T != empty,
	I=(-1..1)*5, I != 0,
	inst(X, Y+I, T),
	inst(X, Y-I, T).

:- not path(n*5, n*5, 0, 0).

is_empty(X, Y) :- inst(X, Y, T), T = empty.
is_empty(X, Y) :- X = -5, Y = (-1..n+1)*5.
is_empty(X, Y) :- X = (n+1)*5, Y = (-1..n+1)*5.
is_empty(X, Y) :- X = (-1..n+1)*5, Y = -5.
is_empty(X, Y) :- X = (-1..n+1)*5, Y = (n+1)*5.



is_cross(X, Y) :- inst(X, Y, T), T = cross.
is_line(X, Y) :- inst(X, Y, T), T = line.
is_corner(X, Y) :- inst(X, Y, T), T = corner.


num_empty(N) :- 
	N = #count{X,Y : is_empty(X, Y)}.



% Make restrictions for the number of instantiated shapes of given types
:- #count{X,Y : is_empty(X,Y)} <= n+2.
:- #count{X,Y : is_cross(X,Y)} > n.
:- #count{X,Y : is_line(X,Y)} > n+1.
:- #count{X,Y : is_corner(X,Y)} > n+1.

% Restrict pieces from being surrounded by empty cells unless it itself is empty
:- not is_empty(X, Y),
	is_empty(X-5, Y), is_empty(X+5, Y), 
	is_empty(X, Y-5), is_empty(X, Y+5). 


#show inst/3.

% #show num_empty/1.
% clingo puzzle.lp -n0 --outf=2 > levels.json

#ifndef VEC_H
#define VEC_H

struct Vec3;

struct Vec2
{
public:
	float x, y;

	Vec2(float x,
		float y);

	Vec2(
		float fillValue);

	Vec2(
		Vec3 vec3);

	~Vec2();
};

struct Vec3
{
public:
	float x, y, z;

	Vec3(float x,
		float y,
		float z);

	Vec3(float fillValue);

	Vec3(
		Vec2 vec2);

	~Vec3();
};

// mph implement later if need be
struct VecPolar
{
	float r, theta;
};

#endif // !VEC_H
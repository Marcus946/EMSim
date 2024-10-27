#include "Vec.h"

Vec2::Vec2(
		float x,
		float y)
		: x(x),
		y(y)
{
}

Vec2::Vec2(
		float fillValue)
		: x(fillValue),
		y(fillValue)
{
}

Vec2::Vec2(
		Vec3 vec3)
		: x(vec3.x),
		y(vec3.y)
{
}

Vec2::~Vec2()
{
}

Vec3::Vec3(
		float x,
		float y,
		float z)
		: x(x),
		y(y),
		z(z)
{
}

Vec3::Vec3(
		float fillValue)
		: x(fillValue),
		y(fillValue),
		z(fillValue)
{
}

Vec3::Vec3(
		Vec2 vec2)
		: x(vec2.x),
		y(vec2.y),
		z(0.0f)
{
}

Vec3::~Vec3()
{
}
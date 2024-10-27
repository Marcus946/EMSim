#include "PointCharge.h"

PointCharge::PointCharge()
		: mChargeNC(1.0f),
		mPosition(0.0f)
{
}

PointCharge::PointCharge(
		const float chargeNC,
		const Vec3 &pos)
		: mChargeNC(chargeNC),
		mPosition(pos)
{
}

PointCharge::PointCharge(
		const PointCharge &other)
		: mChargeNC(other.mChargeNC),
		mPosition(other.mPosition)
{
}

PointCharge::~PointCharge()
{
}

// User is responsible for allocating buffer
bool PointCharge::Serialize(
		void *dest)
{
	if (dest == nullptr) {
		return false;
	}

	unsigned char *pBuffer = (unsigned char *)dest;
	int bytesWritten = 0;

	errno_t status = 0;
	status |= memcpy_s(pBuffer, sizeof(mChargeNC), &mChargeNC, sizeof(mChargeNC));
	bytesWritten += 4;

	status |= memcpy_s(pBuffer + bytesWritten, sizeof(mPosition.x), &mPosition.x, sizeof(mPosition.x));
	bytesWritten += 4;

	status |= memcpy_s(pBuffer + bytesWritten, sizeof(mPosition.y), &mPosition.y, sizeof(mPosition.y));
	bytesWritten += 4;

	status |= memcpy_s(pBuffer + bytesWritten, sizeof(mPosition.z), &mPosition.z, sizeof(mPosition.z));
	bytesWritten += 4;

	return status == 0;
}

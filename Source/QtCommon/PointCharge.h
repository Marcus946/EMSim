#ifndef PointCharge_H
#define PointCharge_H

#include "Vec.h"
#include <qmetatype.h>

namespace MetaType
{
    struct PointCharge
    {
        float mChargeNC;
        Vec3 mPosition;

        PointCharge();
        PointCharge(
            const float chargeNC,
            const Vec3 &pos);
        PointCharge(
            const PointCharge &other);

        ~PointCharge();

        bool Serialize(
            void *buffer);
    };
}

Q_DECLARE_METATYPE(MetaType::PointCharge)

using namespace MetaType;

#endif // !PointCharge_H